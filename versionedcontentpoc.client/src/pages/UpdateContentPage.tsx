import { useState } from "react";
import { Language, useGetApiContentUpdateschema, putApiContentUpdate, putApiContentPublish, putApiContentUnpublish, type UpdateContentRequest, getGetApiContentUpdateschemaQueryKey } from "../api/client";
import { useNavigate, useParams } from 'react-router-dom';
import ContentForm from "../forms/ContentForm";
import { Box, Button, Grid, List, ListItem, ListItemText, Menu, MenuItem, Paper, Tab, Tabs, Typography } from "@mui/material";
import ContentVersionsList from "../compontents/ContentVersionsList";
import { useQueryClient } from '@tanstack/react-query';
import { routes } from "../services/routeResolver";

export default function UpdateContentPage() {
    const { contentId, language } = useParams<{ contentId: string, language: Language }>();
    const queryClient = useQueryClient();
    const { data: response, isLoading, error } = useGetApiContentUpdateschema({ contentId: contentId, language: language });
    const navigate = useNavigate();

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    const languageTabs = Array.from(new Set([...response.data.metadata.languageTranslations!, language]));
    const showAddTranslationButton = languageTabs.length != Object.values(Language).length;
    const contentIsPublished = response.data.metadata.stopPublish === null; //Actually does not check this correctly but will work for now
    const refetch = () => {
        queryClient.invalidateQueries({
            queryKey: getGetApiContentUpdateschemaQueryKey({ contentId, language: language })
        });
    };
    
    return (    
        <Grid container spacing={1} alignItems="flex-start">
            <Grid size={12}>
                <Paper sx={{ p: 1, position: "relative" }}>
                    <Typography
                        variant="h4"
                        component="h1"
                        gutterBottom
                    >
                        Edit Content
                    </Typography>
                    <List dense disablePadding>
                        <ListItem disableGutters>
                            <ListItemText primary="ID" secondary={contentId} />
                        </ListItem>
                        <ListItem disableGutters>
                            <ListItemText primary="Created" secondary={response.data.metadata.created ?? "-"} sx={{m: 0}} />
                        </ListItem>
                        <ListItem disableGutters>
                            <ListItemText primary="Published" secondary={response.data.metadata.startPublish ?? "-"} />
                        </ListItem>
                    </List>
                    <Button
                        variant="contained"
                        color={contentIsPublished ? "error" : "success"}
                        sx={{ position: "absolute", top: 16, right: 16 }}
                        onClick={async () => {
                            const res = contentIsPublished
                                ? await putApiContentUnpublish({ contentId: response.data.metadata.contentId }) 
                                : await putApiContentPublish({ contentId: response.data.metadata.contentId });
                            refetch();
                        }}
                    >
                        {contentIsPublished
                            ? "Unpublish"
                            : "Publish"
                        }
                    </Button>
                </Paper>
            </Grid>
            <Grid size={8}>
                <Paper>
                    <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                        <Tabs
                            value={language}
                            aria-label="basic tabs example"
                            onChange={(e, value) => {
                                navigate(
                                    routes.update.build({
                                        contentId: contentId!,
                                        language: value!
                                    })
                                );
                            }}>
                            {languageTabs.map((languageTranslation) => (
                                <Tab label={languageTranslation} value={languageTranslation} />
                            ))}
                            {showAddTranslationButton &&
                                <DropdownButton
                                    handleSelect={(value) => {
                                        navigate(
                                            routes.update.build({
                                                contentId: contentId!,
                                                language: value!
                                            })
                                        );
                                    }
                                    }
                                    languages={response.data.metadata.languageTranslations}
                                />
                            }
                        </Tabs>
                    </Box>
                    <Box sx={{ p: 1 }}>
                        <UpdateContentForm
                            schema={response.data}
                            onSubmit={refetch}
                            key={response.data.metadata.currentVersionId} />
                    </Box>
                </Paper>
            </Grid>
            <Grid size={4}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        variant="h5"
                        component="h2"
                    >
                        Version history
                    </Typography>
                    <ContentVersionsList
                        contentId={contentId}
                        language={language!}
                        onUpdate={refetch}
                        key={response.data.metadata.currentVersionId} />
                </Paper>
            </Grid>
            
        </Grid>
        
    );
}

interface UpdateContentFormProps {
    schema: UpdateContentRequest;
    onSubmit: () => void;
}
function UpdateContentForm(props: UpdateContentFormProps) {
    const [updateRequest, setUpdateRequest] = useState(props.schema);

    const onSubmit = async () => {
        await putApiContentUpdate(updateRequest);
        props.onSubmit();
    }

    const onChange = (key: string, value: string) => {
        setUpdateRequest((currval) => {
            const newval: UpdateContentRequest = { ...currval };
            newval.propertiesSchema[key].value = value;
            return newval;
        });
    }

    return (
        <ContentForm
            properties={updateRequest.propertiesSchema}
            onSubmit={onSubmit}
            onChange={onChange}
            submitText="Save"
        />
    );
}

interface DropdownButtonProps {
    languages: Language[] | null,
    handleSelect: (value: Language) => void
}
function DropdownButton({ languages, handleSelect }: DropdownButtonProps) {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);


    const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    if (!languages)
        return (<></>);

    return (
        <>
            <Button
                aria-controls={open ? "simple-menu" : undefined}
                aria-haspopup="true"
                onClick={handleClick}
                variant="text"
            >
                Add translation
            </Button>
            <Menu
                id="simple-menu"
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
            >
                {Object.values(Language)
                    .filter(x => !languages?.includes(x))
                    .map(language => (
                        <MenuItem onClick={() => {
                            handleSelect(language);
                            handleClose();
                        }}>
                            {language}
                        </MenuItem>
                ))}
            </Menu>
        </>
    );
}
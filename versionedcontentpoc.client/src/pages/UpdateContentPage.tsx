import { useState } from "react";
import { Language, useGetApiContentUpdateschema, putApiContentUpdate, type UpdateContentRequest, getGetApiContentUpdateschemaQueryKey } from "../api/client";
import { useNavigate, useParams } from 'react-router-dom';
import ContentForm from "../forms/ContentForm";
import { Box, Button, Grid, Menu, MenuItem, Paper, Tab, Tabs, Typography } from "@mui/material";
import ContentVersionsList from "../compontents/ContentVersionsList";
import { useQueryClient } from '@tanstack/react-query';
import { routes } from "../services/routeResolver";

export default function UpdateContentPage() {
    const { contentId, language } = useParams<{ contentId: string, language: Language }>();
    const queryClient = useQueryClient();
    const { data: response, isLoading, error } = useGetApiContentUpdateschema({ contentId: contentId, language: language });
    const navigate = useNavigate();

    const refetch = () => {
        queryClient.invalidateQueries({
            queryKey: getGetApiContentUpdateschemaQueryKey({ contentId, language: language })
        });
    };

    

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    
    const languageTabs = Array.from(new Set([...response.data.metadata.languageTranslations!, language]));
    const showAddTranslationButton = languageTabs.length != Object.values(Language).length;

    return (    
        <Grid container spacing={1}>
            <Grid size={12}>
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
                </Paper>
            </Grid>
            <Grid size={9}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        variant="h1"
                        gutterBottom
                    >
                        Update {contentId}
                    </Typography>
                    <UpdateContentForm
                        schema={response.data}
                        onSubmit={refetch}
                        key={response.data.metadata.currentVersionId} />
                </Paper>
            </Grid>
            <Grid size={3}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        variant="h2"
                    >
                        Versions
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
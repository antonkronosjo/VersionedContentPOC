import { useState } from "react";
import { Language, putApiContentPublish, putApiContentSetasactive, putApiContentUnpublish, putApiContentUpdate, type UpdateContentRequest, type UpdateContentRequestMetadata } from "../../api/client";
import { Box, Button, Grid, List, ListItem, ListItemText, Tab, Tabs, Typography } from "@mui/material";
import ContentForm from "../../forms/ContentForm";
import LanguageSelectButton from "../../compontents/LanguageSelectButton";
import { useNavigate } from "react-router-dom";
import { routes } from "../../utils/routeResolver";
import { relativeDateTime } from "../../utils/dateUtils";

interface EditContentPageHeaderProps {
    metadata: UpdateContentRequestMetadata;
    refetch: () => void;
}
export function EditContentPageHeader({ metadata, refetch }: EditContentPageHeaderProps) {
    const contentIsPublished = metadata.startPublish != null;  //Actually does not check this correctly but will work for now

    return (
        <Grid container>
            <Grid size={6}>
                <Typography variant="h4" component="h1" gutterBottom>
                    Edit Content
                </Typography>
            </Grid>
            <Grid size={6} sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                <Button
                    variant="contained"
                    color={contentIsPublished ? "error" : "success"}
                    sx={{ ml: "auto" }}
                    onClick={async () => {
                        if (contentIsPublished) { await putApiContentUnpublish({ contentId: metadata.contentId }) }
                        else { await putApiContentPublish({ contentId: metadata.contentId }); }
                        refetch();
                    }}
                >
                    {contentIsPublished ? "Unpublish" : "Publish"}
                </Button>
            </Grid>
            <Grid size={12}>
                <List dense disablePadding>
                    <ListItem disableGutters>
                        <ListItemText primary="ID" secondary={metadata.contentId} />
                    </ListItem>
                    <ListItem disableGutters>
                        <ListItemText primary="Created" secondary={relativeDateTime(metadata.created)} sx={{ m: 0 }} />
                    </ListItem>
                    <ListItem disableGutters>
                        <ListItemText primary="Published" secondary={relativeDateTime(metadata.startPublish)} />
                    </ListItem>
                </List>
            </Grid>
        </Grid>
    );
}

interface EditContentFormProps {
    schema: UpdateContentRequest;
    versionId: string | undefined,
    activeVersionId: string | null,
    onSubmit?: () => void;
}
export function EditContentForm({ schema, versionId, activeVersionId, onSubmit }: EditContentFormProps) {
    const [updateRequest, setUpdateRequest] = useState(schema);
    const currentlyEditingActiveVersion = versionId === undefined
            || versionId === activeVersionId; //Todo: can this be done in another way?

    const internalOnSubmit = async () => {
        if (currentlyEditingActiveVersion)
            await putApiContentUpdate(updateRequest);
        else
            await putApiContentSetasactive({ versionId: versionId });

        onSubmit?.();
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
            onSubmit={internalOnSubmit}
            onChange={onChange}
            disabled={!currentlyEditingActiveVersion}
            submitText={currentlyEditingActiveVersion
                ? "Save"
                : "Set as active version"
            }
        />
    );
}

interface LanguageBranchTabsProps {
    metadata: UpdateContentRequestMetadata,
}
export function LanguageBranchTabs({ metadata }: LanguageBranchTabsProps) {
    const navigate = useNavigate();
    const languageTabs = Array.from(new Set([...metadata.languageTranslations!, metadata.language]));
    const showAddTranslationButton = languageTabs.length != Object.values(Language).length;

    return (
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs
                value={metadata.language}
                onChange={(e, value) => {
                    navigate(
                        routes.edit.build({
                            contentId: metadata.contentId!,
                            language: value!
                        })
                    );
                }}>
                {languageTabs.map((languageTranslation) => (
                    <Tab label={languageTranslation} value={languageTranslation} />
                ))}
                {showAddTranslationButton &&
                    <LanguageSelectButton
                        text="Add translation"
                        handleSelect={(value) => {
                            navigate(
                                routes.edit.build({
                                    contentId: metadata.contentId,
                                    language: value!
                                })
                            );
                        }
                        }
                        languages={metadata.languageTranslations}
                    />
                }
            </Tabs>
        </Box>
    );
}
import { useState } from "react";
import { Language, PublishStatus, putApiContentPublish, putApiContentUnpublish, putApiContentUpdate, type PutApiContentUpdate200, type UpdateContentRequest, type UpdateContentRequestMetadata } from "../../api/client";
import { Box, Button, Grid, List, ListItem, ListItemText, Tab, Tabs, Typography } from "@mui/material";
import ContentForm from "../../forms/ContentForm";
import LanguageSelectButton from "../../compontents/LanguageSelectButton";
import { useNavigate } from "react-router-dom";
import { routes } from "../../utils/routeResolver";
import { relativeDateTime } from "../../utils/dateUtils";

interface EditContentPageHeaderProps {
    metadata: UpdateContentRequestMetadata;
    refetch: () => void
}
export function EditContentPageHeader({ metadata, refetch }: EditContentPageHeaderProps) {
    const navigate = useNavigate();
    const contentIsPublished = metadata.status == PublishStatus.Published;

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
                        if (contentIsPublished) {
                            await putApiContentUnpublish({ versionId: metadata.versionId })
                        }
                        else {
                            await putApiContentPublish({ versionId: metadata.versionId });
                        }
                        navigate(routes.edit.build({
                            contentId: metadata.contentId.toString(),
                            language: metadata.language,
                            versionId: metadata.versionId?.toString()
                        }))
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
                        <ListItemText primary="Start publish" secondary={relativeDateTime(metadata.startPublish)} />
                    </ListItem>
                    <ListItem disableGutters>
                        <ListItemText primary="Stop publish" secondary={relativeDateTime(metadata.stopPublish)} />
                    </ListItem>
                </List>
            </Grid>
        </Grid>
    );
}

interface EditContentFormProps {
    schema: UpdateContentRequest;
    onSubmit?: (content: PutApiContentUpdate200) => void;
}
export function EditContentForm({ schema, onSubmit }: EditContentFormProps) {
    const [updateRequest, setUpdateRequest] = useState<UpdateContentRequest>(schema);

    const internalOnSubmit = async () => {
        const res = await putApiContentUpdate(updateRequest);
        onSubmit?.(res.data);
    }

    const onChange = (key: string, value: unknown | undefined) => {
        setUpdateRequest((currval) => {
            const newval: UpdateContentRequest = { ...currval };
            newval.propertiesSchema[key].value = value;
            return newval;
        });
    }

    return (
        <ContentForm
            contentTypeName={updateRequest.metadata.contentTypeName}
            language={updateRequest.metadata.language}
            properties={updateRequest.propertiesSchema}
            onSubmit={internalOnSubmit}
            onChange={onChange}
            submitText={updateRequest.metadata.status === PublishStatus.Draft
                ? "Save"
                : "Save as new draft"
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
                            contentId: metadata.contentId.toString(),
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
                                    contentId: metadata.contentId.toString(),
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
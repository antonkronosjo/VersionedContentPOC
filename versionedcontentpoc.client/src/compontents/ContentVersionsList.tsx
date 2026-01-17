import { Publish, Check, Folder, Home } from "@mui/icons-material";
import { useGetApiContentVersions, putApiContentSetasactive, Language } from "../api/client";
import { Avatar, IconButton, List, ListItem, ListItemAvatar, ListItemButton, ListItemIcon, ListItemText, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { routes } from "../services/routeResolver";
import { Link as RouterLink } from 'react-router-dom';

interface ContentVersionsListProps {
    contentId: string | undefined,
    language: Language,
    versionId: string | undefined,
    onUpdate: () => void
}
export default function ContentVersionsList({ contentId, language, versionId, onUpdate }: ContentVersionsListProps) {
    const { data: response, isLoading, error } = useGetApiContentVersions({ contentId: contentId, language: language });

    if (isLoading)
        return (<p>Is loading</p>);

    if (error || !response)
        return (<p>Error</p>);

    const setAsActiveVersion = async (versionId: string | undefined) => {
        await putApiContentSetasactive({ versionId });
        onUpdate();
    }

    if (!response.data.length)
        return (
            <Typography sx={{ fontStyle: "italic" }}>
                No versions exist for language {language}
            </Typography>
        );

    return (
        <List dense={true}>
            {response.data.map(contentVersion => (
                <ListItemButton
                    selected={contentVersion.versionId == versionId}
                    component={RouterLink}
                    to={routes.update.build({
                        contentId: contentVersion.contentId,
                        language: language,
                        versionId: contentVersion.versionId
                })}>
                    <ListItemIcon>
                        {contentVersion.languageBranch?.activeVersionId === contentVersion.versionId
                            ? <Check color="success" />
                            : <Home />
                        }
                    </ListItemIcon>
                    <ListItemText
                        primary={"ID: " + contentVersion.versionId}
                        secondary={"Created: " + contentVersion.versionCreated}
                    />
                </ListItemButton>
            ))}
        </List>
    );

    return (
        <List dense={true}>
            {response.data.map((contentVersion) => (
                <ListItem key={contentVersion.versionId} secondaryAction={
                    contentVersion.languageBranch?.activeVersionId === contentVersion.versionId
                        ?
                            <IconButton
                                edge="end"
                                aria-label="Published">
                                <Check color="success" />
                            </IconButton>
                        :
                            <IconButton
                                edge="end"
                                aria-label="Ompublicera"
                                onClick={() => {
                                    
                                }}>
                                <Publish />
                            </IconButton>
                }>
                    <ListItemAvatar>
                        <Avatar>
                            <Folder />
                        </Avatar>
                    </ListItemAvatar>
                    <ListItemText
                        primary={contentVersion.versionId}
                        secondary={"Created: " + contentVersion.versionCreated?.split("T")[1]}
                    />
                </ListItem>
            ))}
        </List>
    );
}
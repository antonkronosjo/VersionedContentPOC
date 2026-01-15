import { Publish, Check, Folder } from "@mui/icons-material";
import { useGetApiContentVersions, putApiContentSetasactive, Language } from "../api/client";
import { Avatar, IconButton, List, ListItem, ListItemAvatar, ListItemText, Typography } from "@mui/material";

interface ContentVersionsListProps {
    contentId: string | undefined,
    language: Language,
    onUpdate: () => void
}
export default function ContentVersionsList({ contentId, language,  onUpdate }: ContentVersionsListProps) {
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
                                onClick={() => setAsActiveVersion(contentVersion.versionId)}>
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
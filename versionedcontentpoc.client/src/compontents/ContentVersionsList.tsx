import { Publish, Check, Folder } from "@mui/icons-material";
import { useGetApiContentVersions, putApiContentSetasactive } from "../api/client";
import { Avatar, IconButton, List, ListItem, ListItemAvatar, ListItemText } from "@mui/material";

interface ContentVersionsListProps {
    contentId: string | undefined,
    onUpdate: () => void
}
export default function ContentVersionsList({ contentId, onUpdate }: ContentVersionsListProps) {
    const { data: response, isLoading, error } = useGetApiContentVersions(
        { contentId: contentId, language: 0 },
        { query: { enabled: !!contentId } }
    );

    if (isLoading)
        return (<p>Is loading</p>);

    if (error || !response)
        return (<p>Error</p>);

    const setAsActiveVersion = async (versionId: string | undefined) => {
        await putApiContentSetasactive({ versionId });
        onUpdate();
    }

    return (
        <List dense={true}>
            {response.data.map((contentVersion) => (
                <ListItem secondaryAction={
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
import { CheckCircle, Description, Unpublished, Schedule } from "@mui/icons-material";
import { useGetApiContentVersions, Language, PublishStatus } from "../api/client";
import { List, ListItemButton, ListItemIcon, ListItemText, Typography } from "@mui/material";
import { Link as RouterLink } from 'react-router-dom';
import { routes } from "../utils/routeResolver";
import { relativeDateTime } from "../utils/dateUtils";

interface ContentVersionsListProps {
    contentId: number | undefined,
    language: Language,
    versionId: number | null,
    onUpdate: () => void
}
export default function ContentVersionsList({ contentId, language, versionId }: ContentVersionsListProps) {
    const { data: response, isLoading, error } = useGetApiContentVersions({ contentId: contentId, language: language });

    if (isLoading)
        return (<p>Is loading</p>);

    if (error || !response)
        return (<p>Error</p>);

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
                    key={contentVersion.versionId}
                    selected={contentVersion.versionId == versionId}
                    component={RouterLink}
                    to={routes.edit.build({
                        contentId: contentVersion.contentId.toString(),
                        language: language,
                        versionId: contentVersion.versionId.toString()
                })}>
                    <ListItemIcon>
                        {statusIconMap[contentVersion.status]}
                    </ListItemIcon>
                    <ListItemText
                        primary={"ID: " + contentVersion.versionId}
                        secondary={relativeDateTime(contentVersion.versionCreated)}
                    />
                </ListItemButton>
            ))}
        </List>
    );
}

const statusIconMap = {
    [PublishStatus.Published]: <CheckCircle color="success" />,
    [PublishStatus.Draft]: <Description color="primary" />,
    [PublishStatus.Unpublished]: <Unpublished color="error" />,
    [PublishStatus.DelayedPublish]: <Schedule color="secondary" />
};
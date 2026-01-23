import { Avatar, Card, CardContent, CardHeader, Typography } from "@mui/material";
import type { EventContent, GetApiContentAll200Item, NewsContent } from "../api/client";
import type { JSX } from "react";
import { blue, purple, red } from "@mui/material/colors";
import { relativeDateTime } from "../utils/dateUtils";

interface ContentCardProps {
    content: GetApiContentAll200Item
}
export function ContentCard({ content }: ContentCardProps) {
    return (
        <Card variant="outlined">
            <CardHeader
                avatar={
                    <Avatar
                        sx={{ bgcolor: getContentTypeColor(content.contentType) }}
                        aria-label={content.contentType}
                    >
                        {content.contentType.substring(0, 1)}
                    </Avatar>
                }
                title={content.contentType}
                subheader={"Created: " + relativeDateTime(content.contentRoot?.created)}
            />
            <CardContent>
                {resolveTemplate(content)}
            </CardContent>
        </Card>
    );
}

const resolveTemplate = (content: GetApiContentAll200Item): JSX.Element =>  {
    switch (content.contentType) {
        case "EventContent":
            return <EventTemplate content={content} />;
        case "NewsContent":
            return <NewsTemplate content={content} />;
    }
}

const getContentTypeColor = (contentType:string):string => {
    switch (contentType) {
        case "EventContent":
            return red[500];
        case "NewsContent":
            return purple[500];
        default:
            return blue[500];
    }
}

const NewsTemplate = ({ content }: { content: NewsContent }) => (
    <>
        <Typography variant="h2">{content.heading}</Typography>
        <Typography>{content.lead}</Typography>
    </>
);

const EventTemplate = ({ content }: { content: EventContent }) => (
    <>
        <Typography variant="h2">{content.heading}</Typography>
        <Typography><strong>Start:</strong> {relativeDateTime(content.startDate)}</Typography>
        <Typography><strong>End:</strong> {relativeDateTime(content.endDate)}</Typography>
    </>
);
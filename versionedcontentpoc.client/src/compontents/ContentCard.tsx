import { Avatar, Card, CardActionArea, CardContent, CardHeader, Typography } from "@mui/material";
import type { EventContent, GetApiContentAll200Item, NewsContent } from "../api/client";
import type { JSX } from "react";
import { blue, purple, red } from "@mui/material/colors";
import { relativeDateTime } from "../utils/dateUtils";

type ContentCardProps = {
    readonly content: GetApiContentAll200Item,
    readonly onClick?: (content: GetApiContentAll200Item) => void;
}
export function ContentCard({ content, onClick }: ContentCardProps) {
    return (
        <Card variant="outlined">
            <CardActionArea onClick={() => onClick?.(content)}>
                <CardHeader
                    avatar={
                        <Avatar
                            sx={{ bgcolor: getContentTypeColor(content.contentType) }}
                            aria-label={content.contentType}
                        >
                            {content.contentType?.substring(0, 1)}
                        </Avatar>
                    }
                    title={content.contentType}
                    subheader={"Created: " + relativeDateTime(content.contentRoot?.created)}
                />
                <CardContent>
                    {resolveTemplate(content)}
                </CardContent>
            </CardActionArea>
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
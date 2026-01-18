import { Language, type EventContent, type GetApiContentAll200Item, type NewsContent } from "../../api/client"
import { Link as RouterLink, useSearchParams } from "react-router-dom";
import { IconButton, Avatar, Typography, Card, CardContent, CardHeader, FormControl, InputLabel, Select, MenuItem, FormControlLabel, Checkbox } from "@mui/material";
import { purple, red, blue } from "@mui/material/colors";
import { Edit } from '@mui/icons-material';
import { routes } from "../../services/routeResolver";
import type { JSX } from "react";

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
                action={
                    <IconButton
                        aria-label="edit"
                        component={RouterLink}
                        to={routes.edit.build({
                            contentId: content.contentId,
                            language: content.language
                        })}
                    >

                        {<Edit fontSize="small" />}
                    </IconButton>
                }
                title={content.contentType}
                subheader={"Created:" + content.contentRoot?.created}
            />
            <CardContent>
                {resolveTemplate(content)}
            </CardContent>
        </Card>
    );
}

export function ContentFilter() {
    const [searchParams, setSearchParams] = useSearchParams();
    const language = (searchParams.get("language") as Language) ?? Language.SV;
    const published = searchParams.get("published") === "true";

    const setQueryParam = (key: string, value: string | null) => {
        setSearchParams(prev => {
            const params = new URLSearchParams(prev);
            if (value === null) params.delete(key);
            else params.set(key, value);
            return params;
        });
    };

    return (
        <div style={{ display: "flex", gap: 16, marginTop: 16 }}>
            <FormControl sx={{ minWidth: 200 }}>
                <InputLabel id="language-select-label">View content on language</InputLabel>
                <Select
                    labelId="language-select-label"
                    value={language}
                    onChange={e => setQueryParam("language", e.target.value)}
                    label="View content on language"
                >
                    {Object.values(Language).map(currLang => (
                        <MenuItem key={currLang} value={currLang}>
                            {currLang}
                        </MenuItem>
                    ))}
                </Select>
            </FormControl>
            <FormControlLabel
                control={
                    <Checkbox
                        checked={published}
                        onChange={e => setQueryParam("published", e.target.checked ? "true" : null)}
                    />
                }
                label="Published only"
            />
        </div>
    );
};

const NewsTemplate = ({ content }: { content: NewsContent }) => (
    <>
        <Typography variant="h2">{content.heading}</Typography>
        <Typography>{content.lead}</Typography>
        <Typography>{content.text}</Typography>
    </>
);

const EventTemplate = ({ content }: { content: EventContent }) => (
    <>
        <Typography variant="h2">{content.heading}</Typography>
        <Typography><strong>Start:</strong> {content.startDate}</Typography>
        <Typography><strong>End:</strong> {content.endDate}</Typography>
    </>
);

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
import { useState, type JSX } from "react";
import { Language, useGetApiContentAll, type EventContent, type GetApiContentAll200Item, type NewsContent } from ".././api/client"
import { Link as RouterLink } from "react-router-dom";
import { IconButton, Avatar, Typography, Grid, Card, CardContent, CardHeader, Paper, FormControl, InputLabel, Select, MenuItem } from "@mui/material";
import { purple, red, blue } from "@mui/material/colors";
import { Edit } from '@mui/icons-material';
import { routes } from "../services/routeResolver";


function HomePage() {
    const [language, setLanguage] = useState<Language>(Language.SV);
    const { data, isLoading, error } = useGetApiContentAll({ language: language });
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Paper sx={{ p: 1, width: '75%' }} >
            <Typography variant="h1" gutterBottom>
                HOME - VersionedContentPOC
            </Typography>
            <Grid container spacing={1}>
                <FormControl fullWidth sx={{ mt: 2 }}>
                    <InputLabel id="demo-simple-select-label">View content on language</InputLabel>
                    <Select
                        labelId="demo-simple-select-label"
                        id="demo-simple-select"
                        label="View content on language"
                        value={language}
                        onChange={(e) => { setLanguage(e.target.value) }}
                    >
                        {Object.values(Language).map((currLang) => (
                            <MenuItem value={currLang}>{currLang}</MenuItem>
                        ))}
                    </Select>
                </FormControl>
                {data?.data.map((content) => (
                    <Grid size={12} key={content.contentId}>
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
                                        to={routes.update.build({
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
                    </Grid>
                ))}
            </Grid>
        </Paper>
    );
}

export default HomePage;

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
import type { JSX } from "react";
import { useGetApiContentAll, type EventContent, type GetApiContentAll200Item, type NewsContent } from ".././api/client"
import { Link as RouterLink } from "react-router-dom";
//import EditIcon from '@mui/icons-material/Edit';
import { IconButton, Avatar, Typography, Grid, Card, CardContent, CardHeader, Paper } from "@mui/material";
import { purple, red, blue } from "@mui/material/colors";
import { Edit } from '@mui/icons-material';


function HomePage() {
    const { data, isLoading, error } = useGetApiContentAll();
    
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
                                        to={"/update/" + content.contentId}
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
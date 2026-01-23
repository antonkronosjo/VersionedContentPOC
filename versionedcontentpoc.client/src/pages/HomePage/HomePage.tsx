import { Language, useGetApiContentAll } from "../../api/client"
import { useSearchParams } from "react-router-dom";
import { Typography, Grid, Paper } from "@mui/material";
import { ContentCard, ContentFilter } from "./HomePageComponents";


function HomePage() {
    const [searchParams] = useSearchParams();
    const language = searchParams.get("language") as Language ?? Language.SV;
    const { data, isLoading, error } = useGetApiContentAll({ language: language, published: true });
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Grid container>
            <Grid size={6}>
                <Paper sx={{ p: 1 }} >
                    <Typography variant="h1" gutterBottom>
                        HOME - VersionedContentPOC
                    </Typography>
                    <Grid container spacing={1}>
                        <Grid size={12}>
                            <ContentFilter />
                        </Grid>
                        {data?.data.map((content) => (
                            <Grid size={12} key={content.contentId}>
                                <ContentCard content={content} />
                            </Grid>
                        ))}
                    </Grid>
                </Paper>
            </Grid>

        </Grid>
    );
}

export default HomePage;
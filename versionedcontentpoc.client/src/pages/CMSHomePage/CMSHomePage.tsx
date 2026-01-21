import { useGetApiContentsummaryContentroots } from "../../api/client"
import { useSearchParams } from "react-router-dom";
import { Typography, Grid, Paper, Box } from "@mui/material";
import { ContentFilter, ContentRootTable } from "./CmsHomePageComponents";


export default function CMSHomePage() {
    const [searchParams] = useSearchParams();
    const contentType = searchParams.get("contentType") as string | undefined;
    const published = searchParams.get("published") === "true";
    const language = searchParams.get("language");
    const { data, isLoading, error } = useGetApiContentsummaryContentroots({ ContentType: contentType, Published: published, Language: language });
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Grid container>
            <Grid size={9}>
                <Paper sx={{ p: 1 }} >
                    <Typography variant="h4" component="h1" gutterBottom>
                        Manage content
                    </Typography>
                    <ContentFilter />
                    <Box sx={{ pt: 2 }}>
                        {data?.data?.length
                            ? <ContentRootTable contentRoots={data?.data} />
                            : <i>No content exists...</i>
                        }
                    </Box>
                </Paper>
            </Grid>
        </Grid>
    );
}
import { useState } from "react";
import { Language, useGetApiContentCreationschema, postApiContentCreate, type CreateContentRequest, useGetApiContentTypes } from "../../api/client";
import { useParams, useSearchParams } from 'react-router-dom';
import ContentForm from "../../forms/ContentForm";
import { useNavigate } from 'react-router-dom';
import { Grid, Paper, Typography } from "@mui/material";
import { routes } from "../../services/routeResolver";
import { ContentTypeSelect, CreateContentForm } from "./CreateContentPageComponents";

export default function CreateContentPage() {
    const { data: response, isLoading, error } = useGetApiContentTypes();
    const [searchParams] = useSearchParams();
    const language = searchParams.get("language") as Language ?? Language.SV;
    const contentType = searchParams.get("contentType");

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Grid container spacing={1}>
            <Grid size={9}>
                <Paper sx={{ p: 1 }}>
                    <Typography variant="h1" gutterBottom>
                        Select content type and language
                    </Typography>
                    <ContentTypeSelect
                        contentTypes={response.data}
                    />
                </Paper>
            </Grid>
            {contentType && language &&
                <Grid size={9}>
                    <Paper sx={{ p: 1 }}>
                        <CreateContentForm contentType={contentType} language={language} key={contentType + language} />
                    </Paper>
                </Grid>
            }
        </Grid>
    );
}
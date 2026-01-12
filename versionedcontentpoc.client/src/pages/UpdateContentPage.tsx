import { useState } from "react";
import { useGetApiContentUpdateschema, putApiContentUpdate, type UpdateContentRequest, getGetApiContentUpdateschemaQueryKey, type Language } from "../api/client";
import { useParams } from 'react-router-dom';
import ContentForm from "../forms/ContentForm";
import { Grid, Paper, Typography } from "@mui/material";
import ContentVersionsList from "../compontents/ContentVersionsList";
import { useQueryClient } from '@tanstack/react-query';

export default function UpdateContentPage() {
    const { contentId, language } = useParams<{ contentId: string, language: Language }>();
    const queryClient = useQueryClient();
    const { data: response, isLoading, error } = useGetApiContentUpdateschema({ contentId: contentId, language: language });

    const refetch = () => {
        queryClient.invalidateQueries({
            queryKey: getGetApiContentUpdateschemaQueryKey({ contentId, language: language })
        });
    };

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (    
        <Grid container spacing={1}>
            <Grid size={9}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        variant="h1"
                        gutterBottom
                    >
                        Update {contentId}
                    </Typography>
                    <UpdateContentForm
                        schema={response.data}
                        onSubmit={refetch}
                        key={response.data.metadata.currentVersionId} />
                </Paper>
            </Grid>
            <Grid size={3}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        variant="h2"
                    >
                        Versions
                    </Typography>
                    <ContentVersionsList
                        contentId={contentId}
                        language={language}
                        onUpdate={refetch}
                        key={response.data.metadata.currentVersionId} />
                </Paper>
            </Grid>
            
        </Grid>
        
    );
}

interface UpdateContentFormProps {
    schema: UpdateContentRequest;
    onSubmit: () => void;
}
function UpdateContentForm(props: UpdateContentFormProps) {
    const [updateRequest, setUpdateRequest] = useState(props.schema);

    const onSubmit = async () => {
        await putApiContentUpdate(updateRequest);
        props.onSubmit();
    }

    const onChange = (key: string, value: string) => {
        setUpdateRequest((currval) => {
            const newval: UpdateContentRequest = { ...currval };
            newval.propertiesSchema[key].value = value;
            return newval;
        });
    }

    return (
        <ContentForm
            properties={updateRequest.propertiesSchema}
            onSubmit={onSubmit}
            onChange={onChange}
            submitText="Save"
        />
    );
}
import { useState } from "react";
import { useGetApiContentUpdateschema, putApiContentUpdate, type UpdateContentRequest } from "../api/client";
import { useNavigate, useParams } from 'react-router-dom';
import ContentForm from "../forms/ContentForm";
import { Grid, Paper, Typography } from "@mui/material";

export default function UpdateContentPage() {
    const { contentId } = useParams<{ contentId: string }>();

    const { data: response, isLoading, error } = useGetApiContentUpdateschema(
        { contentId: contentId, language: 0 },
        { query: { enabled: !!contentId } }
    );

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Grid container spacing={1}>
            <Grid size={9}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        mb={1}
                    >
                        Update {contentId}
                    </Typography>
                    <UpdateContentForm schema={response.data} />
                </Paper>
            </Grid>
            <Grid size={3}>
                <Paper sx={{ p: 1 }}>
                    <Typography
                        mb={1}
                    >
                        Content versions
                    </Typography>
                    fafaf
                    fafa
                </Paper>
            </Grid>
            
        </Grid>
        
    );
}

interface UpdateContentFormProps {
    schema: UpdateContentRequest;
}
function UpdateContentForm(props: UpdateContentFormProps) {
    const [updateRequest, setUpdateRequest] = useState(props.schema);
    const navigate = useNavigate();

    const onSubmit = async () => {
        await putApiContentUpdate(updateRequest);
        navigate("/");
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
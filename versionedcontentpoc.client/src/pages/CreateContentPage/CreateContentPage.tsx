import { useState } from "react";
import { useGetApiContentCreationschema, postApiContentCreate, type CreateContentRequest, type Language } from "../../api/client";
import { useParams } from 'react-router-dom';
import ContentForm from "../../forms/ContentForm";
import { useNavigate } from 'react-router-dom';
import { Paper, Typography } from "@mui/material";
import { routes } from "../../services/routeResolver";

function CreateContentPage() {
    const { contentType, language } = useParams<{ contentType: string, language: Language }>();
    const { data: response, isLoading, error } = useGetApiContentCreationschema(
        { contentTypeName: contentType, language: language },
        { query: { enabled: !!contentType } }
    );

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <>
            <Paper sx={{ p: 1, width: "75%" }}>
                <Typography
                    variant="h1"
                    gutterBottom
                >
                    Create new {contentType} for language "{language}"
                </Typography>
                <CreateContentForm schema={response.data} />
            </Paper>
        </>
    );
}

interface CreateContentFormProps {
    schema: CreateContentRequest;
}
function CreateContentForm(props: CreateContentFormProps) {
    const [createRequest, setCreateRequest] = useState(props.schema);
    const navigate = useNavigate();

    const onSubmit = async () => {
        const response = await postApiContentCreate(createRequest);
        navigate(routes.edit.build({
            contentId: response.data.contentId,
            language: response.data.language
        }));
    }

    const onChange = (key: string, value: string) => {
        setCreateRequest((currval) => {
            const newval: CreateContentRequest = { ...currval };
            newval.propertiesSchema[key].value = value;
            return newval;
        });
    }

    return (
        <ContentForm
            properties={createRequest.propertiesSchema}
            onSubmit={onSubmit}
            onChange={onChange}
            submitText="Create content"
            />
    );
}

export default CreateContentPage;
import { useState } from "react";
import { useGetApiContentUpdateschema, putApiContentUpdate, type UpdateContentRequest } from "../api/client";
import { useParams } from 'react-router-dom';
import ContentForm from "../forms/ContentForm";

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
        <>
            <h1>Update {contentId}</h1>
            <UpdateContentForm schema={response.data} />
        </>
        
    );
}

interface UpdateContentFormProps {
    schema: UpdateContentRequest;
}
function UpdateContentForm(props: UpdateContentFormProps) {
    const [updateRequest, setUpdateRequest] = useState(props.schema);

    const onSubmit = async () => {
        await putApiContentUpdate(updateRequest);
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
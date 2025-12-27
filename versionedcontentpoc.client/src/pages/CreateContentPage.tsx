import { useEffect } from "react";
import { useGetApiContentCreationschema, type CreateContentRequestPropertiesSchema } from "../api/client";
import { useParams } from 'react-router-dom';

function CreateContentPage() {
    const { contentType } = useParams<{ contentType: string }>();
    const { data: response, isLoading, error } = useGetApiContentCreationschema(
        { contentTypeName: contentType, language: 0 },
        { query: { enabled: !!contentType } }
    );

    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    console.warn("SCHEMA", response.data); //<-- data.data is a string

    return (
        <>
            <h1>Create content type</h1>
            <CreateForm schema={response.data.propertiesSchema} />
        </>
        
    );
}

interface CreateFormProps {
    // Use indexed access to get the specific type of propertiesSchema
    schema: CreateContentRequestPropertiesSchema;
}
function CreateForm(props: CreateFormProps) {
    if (!props.schema)
        return (<p>Schema is null!</p>);

    console.log(Object.keys(props.schema));

    return (
        <>
            {Object.entries(props.schema).map(([key, prop]) => (
                <div key={key}>
                    <label>{key}</label>
                    <input />
                   
                </div>
            ))}
        </>
    );
}

export default CreateContentPage;
import { useGetApiContentCreationschema, type CreateContentRequestPropertiesSchema } from "../api/client";
import { useParams } from 'react-router-dom';

function CreateContentPage() {
    const { contentType } = useParams<{ contentType: string }>();
    const { data, isLoading, error } = useGetApiContentCreationschema(
        { contentTypeName: contentType, language: 0 },
        { query: { enabled: !!contentType } }
    );

    

    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    console.warn(JSON.parse(data.data));

    return (
        <>
            <h1>Create content type</h1>
            <CreateForm schema={data?.data?.propertiesSchema} />
        </>
        
    );
}

function CreateForm(schema: CreateContentRequestPropertiesSchema) {
    if (!schema)
        return (<p>Schema is null!</p>);

    console.log(schema);

    return (
        <>
            {Object.entries(schema).map(([key, prop]) => (
                <>
                    <label>{key}</label>
                    <input />
                </>
            ))}
        </>
    );
}

export default CreateContentPage;
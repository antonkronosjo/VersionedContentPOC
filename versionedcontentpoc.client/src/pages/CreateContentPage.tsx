import { useState } from "react";
import {
    useGetApiContentCreationschema,
    type CreateContentRequestPropertiesSchema,
    postApiContentCreate,
    type CreateContentRequest
} from "../api/client";
import { useParams } from 'react-router-dom';

function CreateContentPage() {
    const { contentType } = useParams<{ contentType: string }>();
    const { data: response, isLoading, error } = useGetApiContentCreationschema(
        { contentTypeName: contentType, language: 0 },
        { query: { enabled: !!contentType } }
    );

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <>
            <h1>Create content type</h1>
            <CreateForm schema={response.data} />
        </>
        
    );
}

interface CreateFormProps {
    schema: CreateContentRequest;
}
function CreateForm(props: CreateFormProps) {
    const [createRequest, setCreateRequest] = useState(props.schema);

    const onSubmit = async () => {

        const createdContent = await postApiContentCreate(createRequest);
    }

    return (
        <>
            {Object.entries(createRequest.propertiesSchema).map(([key, prop]) => (
                <div key={key}>
                    <label>{key}</label>
                    <input onChange={(e) => {
                        const inputValue = e.target.value;
                        
                        setCreateRequest((currVal) => {
                            const newVal = { ...currVal };
                            newVal.propertiesSchema[key].value = inputValue;
                            return newVal;
                        });
                    }}
                        value={createRequest.propertiesSchema[key].value ?? ""} />
                   
                </div>
            ))}
            <button onClick={onSubmit}>Save</button>
        </>
    );
}

export default CreateContentPage;
import { useEffect, useState } from "react";
import { Language, useGetApiContentCreationschema, postApiContentCreate, type CreateContentRequest } from "../../api/client";
import { data, useParams, useSearchParams } from 'react-router-dom';
import ContentForm from "../../forms/ContentForm";
import { useNavigate } from 'react-router-dom';
import { FormControl, InputLabel, MenuItem, Paper, Select, Typography } from "@mui/material";
import { routes } from "../../services/routeResolver";

interface ContentTypeSelectProps {
    contentTypes: string[]
}
export function ContentTypeSelect({ contentTypes }: ContentTypeSelectProps) {
    const [searchParams, setSearchParams] = useSearchParams();
    const language = searchParams.get("language") as Language ?? Language.SV;
    const contentType = searchParams.get("contentType");

    const setQueryParam = (key: string, value: string | null) => {
        setSearchParams(prev => {
            const params = new URLSearchParams(prev);
            if (value === null) params.delete(key);
            else params.set(key, value);
            return params;
        });
    };

    return (
        <>
            <FormControl fullWidth>
                <InputLabel id="demo-simple-select-label1">Content type</InputLabel>
                <Select
                    labelId="demo-simple-select-label1"
                    id="demo-simple-select1"
                    label="Select content type"
                    value={contentType}
                    variant="filled"
                    onChange={(e) => { setQueryParam("contentType", e.target.value) }}
                >
                    {contentTypes.map((contentType) => (
                        <MenuItem value={contentType}>{contentType}</MenuItem>
                    ))}
                </Select>
            </FormControl>
            <FormControl fullWidth sx={{ mt: 2 }} >
                <InputLabel id="demo-simple-select-label">Language</InputLabel>
                <Select
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    label="Language"
                    value={language}
                    variant="filled"
                    onChange={(e) => { setQueryParam("language", e.target.value) }}
                >
                    {Object.values(Language).map((currLang) => (
                        <MenuItem value={currLang}>{currLang}</MenuItem>
                    ))}
                </Select>
            </FormControl>
        </>
    );
}

interface CreateContentFormProps {
    contentType: string,
    language: Language,
}
export function CreateContentForm({ contentType, language }: CreateContentFormProps) {
    const navigate = useNavigate();
    const [createRequest, setCreateRequest] = useState(null);
    const { data: response, isLoading, error } = useGetApiContentCreationschema(
        { contentTypeName: contentType, language: language }
    );
    

    useEffect(() => {
        if (response?.data) {
            setCreateRequest(response.data);
        }
    }, [response?.data]);

    if (isLoading || !response || !createRequest)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);
    

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
            disabled={false}
        />
    );
}
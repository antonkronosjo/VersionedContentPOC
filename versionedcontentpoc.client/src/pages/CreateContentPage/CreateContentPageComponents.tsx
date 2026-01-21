import { useEffect, useState } from "react";
import { Language, useGetApiContentCreationschema, postApiContentCreate, type CreateContentRequest } from "../../api/client";
import { useSearchParams } from 'react-router-dom';
import ContentForm from "../../forms/ContentForm";
import { useNavigate } from 'react-router-dom';
import { routes } from "../../services/routeResolver";
import LanguageSelector from "../../formElements/LanguageSelector";
import ContentTypeSelector from "../../formElements/ContentTypeSelector";
import { Grid } from "@mui/material";

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
        <Grid container spacing={2}>
            <Grid size={12}>
                <ContentTypeSelector value={contentType} contentTypes={contentTypes} onChange={(e) => { setQueryParam("contentType", e.target.value) }} />
            </Grid>
            <Grid size={12}>
                <LanguageSelector value={language} onChange={(e) => { setQueryParam("language", e.target.value) }} />
            </Grid>
        </Grid>
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
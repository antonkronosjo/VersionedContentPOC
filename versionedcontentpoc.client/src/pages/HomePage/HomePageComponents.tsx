import { Language, useGetApiContentTypes } from "../../api/client"
import { Link as RouterLink, useSearchParams } from "react-router-dom";
import { Grid } from "@mui/material";
import LanguageSelector from "../../formElements/LanguageSelector";
import ContentTypeSelector from "../../formElements/ContentTypeSelector";

export function ContentFilter() {
    const [searchParams, setSearchParams] = useSearchParams();
    const language = (searchParams.get("language") as Language) ?? Language.SV;
    // const contentType = searchParams.get("contentType");
    // const { data: response, isLoading, error } = useGetApiContentTypes();

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
            <Grid size={6}>
                <LanguageSelector value={language} onChange={e => setQueryParam("language", e.target.value)} />
            </Grid>
            {/* <Grid size={6}>
                <ContentTypeSelector value={language} contentTypes={response?.data ?? []} onChange={e => setQueryParam("contentType", e.target.value)} />
            </Grid> */}
        </Grid>
    );
};
import { Language, type EventContent, type GetApiContentAll200Item, type NewsContent } from "../../api/client"
import { Link as RouterLink, useSearchParams } from "react-router-dom";
import { IconButton, Avatar, Typography, Card, CardContent, CardHeader, FormControl, InputLabel, Select, MenuItem, FormControlLabel, Checkbox, Grid } from "@mui/material";
import { purple, red, blue } from "@mui/material/colors";
import { Edit } from '@mui/icons-material';
import { routes } from "../../services/routeResolver";
import type { JSX } from "react";
import LanguageSelector from "../../formElements/LanguageSelector";
import { formatDateString } from "../CMSHomePage/CmsHomePageComponents";

export function ContentFilter() {
    const [searchParams, setSearchParams] = useSearchParams();
    const language = (searchParams.get("language") as Language) ?? Language.SV;

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
        </Grid>
    );
};
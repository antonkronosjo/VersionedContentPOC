import { Button, FormControl, Grid, InputLabel, MenuItem, Paper, Select, Typography } from "@mui/material";
import { Language, useGetApiContentTypes } from "../api/client"
import { useState } from "react";
import { Link as RouterLink } from 'react-router-dom';
import { routes } from "../services/routeResolver";

function SelectContentPage() {
    const { data, isLoading, error } = useGetApiContentTypes();
    const [contentType, setContentType] = useState("");
    const [language, setLanguage] = useState<Language>(Language.SV);
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Paper sx={{p: 1, width: "75%"}}>
            <Typography
                variant="h1"
                gutterBottom
            >
                Create new content
            </Typography>
            <Grid container spacing={1}>
                <Grid size={12}>
                    <FormControl fullWidth>
                        <InputLabel id="demo-simple-select-label1">Content type</InputLabel>
                        <Select
                            labelId="demo-simple-select-label1"
                            id="demo-simple-select1"
                            label="Select content type"
                            value={contentType}
                            onChange={(e) => { setContentType(e.target.value) }}
                        >
                            {data?.data.map((contentType) => (
                                <MenuItem value={contentType}>{contentType}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <FormControl fullWidth sx={{ mt: 2 }}>
                        <InputLabel id="demo-simple-select-label">Language</InputLabel>
                        <Select
                            labelId="demo-simple-select-label"
                            id="demo-simple-select"
                            label="Language"
                            value={language}
                            onChange={(e) => { setLanguage(e.target.value) }}
                        >
                            {Object.values(Language).map((currLang) => (
                                <MenuItem value={currLang}>{currLang}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Grid>
                <Button
                    component={RouterLink}
                    to={routes.create.build({ contentType: contentType, language: language })}
                    disabled={!contentType}
                    sx={{ ml: 'auto' }}
                    variant="contained">
                    Select
                </Button>
            </Grid>

        </Paper>
        
    );
}

export default SelectContentPage;
import { Button, FormControl, Grid, InputLabel, MenuItem, Paper, Select, Typography } from "@mui/material";
import { useGetApiContentTypes } from "../api/client"
import { useState } from "react";
import { Link as RouterLink } from 'react-router-dom';

function SelectContentPage() {
    const { data, isLoading, error } = useGetApiContentTypes();
    const [contentType, setContentType] = useState("");
    
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
                Select content type
            </Typography>
            <Grid container spacing={1}>
                <Grid size={12}>
                    <FormControl fullWidth>
                        <InputLabel id="demo-simple-select-label">Select content type</InputLabel>
                        <Select
                            labelId="demo-simple-select-label"
                            id="demo-simple-select"
                            label="Select content type"
                            value={contentType}
                            onChange={(e) => { setContentType(e.target.value) }}
                        >
                            {data?.data.map((contentType) => (
                                <MenuItem value={contentType}>{contentType}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Grid>
                <Button
                    component={RouterLink}
                    to={"/create/" + contentType}
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
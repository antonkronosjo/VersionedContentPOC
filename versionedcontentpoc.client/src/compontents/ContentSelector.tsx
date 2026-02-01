import { Button, Grid, InputAdornment, TextField } from "@mui/material";
import { useGetApiContentAll, type GetApiContentAll200Item, type Language } from "../api/client";
import { ContentCard } from "./ContentCard";
import { useEffect, useRef, useState } from "react";
import { Search } from "@mui/icons-material";

type ContentSelectorProps = {
    readonly language: Language;
    readonly onSelect: (content: GetApiContentAll200Item) => void;
}
export default function ContentSelector({ language, onSelect }: ContentSelectorProps) {
    const { data, isLoading, error } = useGetApiContentAll({ language: language, published: false });
    const [nameFilter, setNameFilter] = useState("");
    const inputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        inputRef.current?.focus(); // focus on mount
    }, []);

    const contentFilter = (content: GetApiContentAll200Item) => {
        if (!nameFilter)
            return true;

        return content.heading.toLowerCase().includes(nameFilter.toLowerCase());
    }

    if (!data || isLoading || error)
        return (<>Something went wrong.</>);

    return (
        <Grid container spacing={2}>
            <Grid size={12}>
                <TextField inputRef={inputRef}
                    fullWidth
                    placeholder="Search for content..."
                    value={nameFilter} onChange={(e) => setNameFilter(e.target.value)}
                    slotProps={{
                        input: {
                            startAdornment: (
                                <InputAdornment position="start">
                                    <Search />
                                </InputAdornment>
                            ),
                        },
                    }}
                />
            </Grid>
            {data.data.filter(contentFilter).map(content => (
                <Grid size={3}>
                    <ContentCard content={content} onClick={onSelect} />
                </Grid>
            ))}
        </Grid>
    );
}
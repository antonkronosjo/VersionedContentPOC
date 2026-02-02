import { Button, Chip, Dialog, DialogContent, DialogTitle, FormControl, IconButton, InputAdornment, InputLabel, Paper, TextField, Typography, type TextFieldProps } from "@mui/material";
import { useEffect, useState } from "react";
import { getApiContentGet, useGetApiContentGet, type ContentReference, type Language } from "../api/client";
import ContentSelector from "../compontents/ContentSelector";
import { ArrowDropDown, Clear, Close } from "@mui/icons-material";

type ContentPickerProps = {
    value: ContentReference | undefined;
    label: string;
    language: Language;
    onChange: (value: ContentReference | undefined) => void;
}
export default function ContentPicker({ label, value, language, onChange }: ContentPickerProps) {
    const [open, setOpen] = useState(false);
    const [displayName, setDisplayName] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    //const loadDisplayName = async (contentId?: string) => {
    //    if (!contentId) return;

    //    setLoading(true);

    //};

    useEffect(() => {
        getApiContentGet({ contentId: value?.contentId, language: language, published: false }).then((res) => {
            console.warn("RES", res);
            setDisplayName([
                res.data.heading,
                "(" + res.data.contentId + ")"
            ].join(" "));
            setLoading(false);
        });
    }, [value?.contentId]);

    return (
        <>

            <TextField
                label={label}
                value={displayName ? "" : ""}
                variant="filled"
                fullWidth
                sx={{ cursor: "pointer" }}
                focused={open ? open : undefined}
                slotProps={{
                    input: {
                        sx: {
                            caretColor: "transparent",
                            cursor: "pointer",
                            userSelect: "none",
                        },
                        readOnly: true,
                        startAdornment: value ? (
                            <InputAdornment position="start">
                                <Chip
                                    color="primary"
                                    label={displayName}
                                    size="small"
                                    onDelete={() => onChange(undefined)}
                                    onMouseDown={(e) => e.stopPropagation()} // prevent focus issues
                                />
                            </InputAdornment>
                        ) : undefined
                    }
                }}
                onClick={() => setOpen(true)}
            />

            <Dialog open={open} onClose={() => setOpen(false)} maxWidth={false}>
                <Paper sx={{
                    width: 1280, // set custom width in pixels
                    maxWidth: '90vw', // optional: responsive
                    height: 1280,
                    maxHeight: "90vh"
                }}>
                    <DialogTitle sx={{ m: 0, p: 2 }}>
                        <Typography variant="h6">Select content</Typography>
                        <IconButton
                            aria-label="close"
                            onClick={() => setOpen(false)}
                            sx={{
                                position: "absolute",
                                right: 8,
                                top: 8
                            }}
                        >
                            <Close />
                        </IconButton>
                    </DialogTitle>
                    <DialogContent>
                        <ContentSelector language={language} onSelect={(content) => {
                            onChange({ contentId: content.contentId });
                            setOpen(false);
                        }} />
                    </DialogContent>
                </Paper>
            </Dialog>
        </>
    )
}
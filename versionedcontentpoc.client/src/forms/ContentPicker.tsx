import { Box, Button, Chip, Dialog, DialogContent, DialogTitle, FormControl, IconButton, InputLabel, Paper, TextField, Typography } from "@mui/material";
import { useState } from "react";
import type { ContentReference, Language } from "../api/client";
import ContentSelector from "../compontents/ContentSelector";
import { Close } from "@mui/icons-material";

type ContentPickerProps = {
    value: ContentReference | undefined;
    language: Language;
    onChange: (value: ContentReference | undefined) => void;
}
export default function ContentPicker({ value, language, onChange }: ContentPickerProps) {
    const [open, setOpen] = useState(false);
    const [focused, setFocused] = useState(false);
    const hasValue = value && value.contentId > 0;
     
    return (
        <>


            <FormControl variant="outlined" fullWidth>
                <InputLabel shrink={hasValue} htmlFor="custom-button">
                    Select content
                </InputLabel>

                {/*<Box*/}
                {/*    id="custom-button"*/}
                {/*    component={Button}*/}
                {/*    variant="outlined"*/}
                {/*    onClick={() => alert("Clicked!")}*/}
                {/*    sx={{*/}
                {/*        height: 56,*/}
                {/*        justifyContent: "flex-start",*/}
                {/*        padding: "16.5px 14px",*/}
                {/*        textTransform: "none",*/}
                {/*    }}*/}
                {/*>*/}
                    
                {/*</Box>*/}
                <TextField />
                {hasValue &&
                    <Chip
                        label={value.contentId}
                        onDelete={() => onChange(undefined)}
                    />
                }
                {!hasValue &&
                    <Button onClick={() => setOpen(true)}>
                        Select content
                    </Button>
                }
            </FormControl>

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
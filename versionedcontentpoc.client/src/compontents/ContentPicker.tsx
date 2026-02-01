import { Button, Chip, Dialog, TextField, type TextFieldProps } from "@mui/material";
import { useState } from "react";
import type { ContentReference } from "../api/client";

type ContentPickerProps = {
    value: ContentReference | undefined,
    onChange: (value: number | undefined) => void,
}
export default function ContentPicker({ value, onChange }: ContentPickerProps) {
    const [open, setOpen] = useState(false);
     
    return (
        <>
            {value &&
                <Chip
                    label={value.contentId}
                onDelete={() => onChange({ target: { value: undefined } })}
                />
            }
            {!value &&
                <Button onClick={() => setOpen(true) }>
                    Select a value
                </Button>
            }
            
            <Dialog open={open} onClose={() => setOpen(false)}>
                <ContentPicker2 onSelect={(value: number) => {
                    setOpen(false);
                    onChange?.({ target: { value: value } });
                }} />
            </Dialog>
        </>
    )
}

type ContentPicker2Props = {
    onSelect: (value: ContentReference) => void;
}
function ContentPicker2({ onSelect }: ContentPicker2Props) {
    return (
        <>
            <Button onClick={() => onSelect({contentId: 1})}>
                1
            </Button>
            <Button onClick={() => onSelect({contentId: 2})}>
                2
            </Button>
        </>
    );
}
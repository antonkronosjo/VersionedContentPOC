import { Button, Chip, Dialog, TextField, type TextFieldProps } from "@mui/material";
import { useState } from "react";

type ContentPickerProps = {
    value: number | undefined,
    onChange: (value: number | undefined) => void,
}
export default function ContentPicker({ value, onChange }: ContentPickerProps) {
    const [open, setOpen] = useState(false);
     
    return (
        <>
            {value &&
                <Chip
                    label={value}
                    onDelete={() => onChange(undefined)}
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
                    onChange?.(value);
                }} />
            </Dialog>
        </>
    )
}

type ContentPicker2Props = {
    onSelect: (value:number) => void;
}
function ContentPicker2({ onSelect }: ContentPicker2Props) {
    return (
        <>
            <Button onClick={() => onSelect(1)}>
                1
            </Button>
            <Button onClick={() => onSelect(2)}>
                2
            </Button>
        </>
    );
}
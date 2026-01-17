import type { ChangeEventHandler, JSX } from "react";
import { InputType, type ContentPropertyValueDto } from "../api/client";
import { Button, Grid, TextField, type TextFieldProps, type TextFieldVariants } from "@mui/material";

interface ContentFormProps {
    properties: { [key: string]: ContentPropertyValueDto };
    disabled: boolean;
    onChange: (key: string, value: string) => void;
    onSubmit: () => Promise<void>;
    submitText: string;
}
export default function ContentForm({ properties, onChange, onSubmit, submitText, disabled }: ContentFormProps) {
    return (
        <form onSubmit={(e) => {
            e.preventDefault();
        }}>
            <Grid container spacing={2}>
                {Object.entries(properties).map(([key]) => (
                    <Grid size={12} key={key}>
                        {resolveTemplate(
                            key,
                            properties[key],
                            disabled,
                            (e) => { onChange(key, e.target.value) })
                        }
                    </Grid>
                ))}
                <Button sx={{ ml: 'auto' }} variant="contained" onClick={onSubmit}>
                    {submitText}
                </Button>
            </Grid>
        </form>
    );
}

const resolveTemplate = (label: string, propertyValue: ContentPropertyValueDto, formDisabled:boolean, onChange: ChangeEventHandler<HTMLInputElement>): JSX.Element => {
    const baseProps = {
        label: label,
        variant: "filled",
        value: propertyValue.value?.toString() ?? "",
        onChange: onChange,
        fullWidth: true,
        disabled: propertyValue.readOnly || formDisabled,
        //slotProps: {
        //    input: {
        //        readOnly: propertyValue.readOnly
        //    }
        //}
    } as TextFieldProps;

    switch (propertyValue.inputType) {
        case InputType.Input:
            return <TextField {...baseProps} />
        case InputType.TextArea:
            return <TextField {...baseProps} multiline rows={7} />
        case InputType.DateTimePicker:
            return <TextField {...baseProps} type="datetime-local" />
        
        default:
            return <>No template defined for content type</>
    }
}
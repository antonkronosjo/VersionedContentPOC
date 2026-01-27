import { useEffect, useState, type ChangeEventHandler, type JSX } from "react";
import { InputType, postApiValidationProperty, type ContentPropertyValueDto, type ValidationResult } from "../api/client";
import { Button, Grid, TextField, type TextFieldProps } from "@mui/material";
import useUpdateEffect from "../hooks/useUpdateEffect";
import { useDebouncedCallback } from "../hooks/useDebouncedCallback";

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
                        <FormElementTemplate
                            label={key}
                            propertyName={key}
                            valueDto={properties[key]}
                            disabled={disabled}
                            onChange={e => onChange(key, e.target.value)}
                        />
                    </Grid>
                ))}
                <Button sx={{ ml: 'auto' }} variant="contained" onClick={onSubmit}>
                    {submitText}
                </Button>
            </Grid>
        </form>
    );
}


type FormElementTemplateProps = {
    label: string;
    propertyName: string;
    valueDto: ContentPropertyValueDto;
    disabled: boolean;
    onChange: ChangeEventHandler<HTMLInputElement>;
}
function FormElementTemplate({ label, propertyName, valueDto, disabled, onChange }: FormElementTemplateProps) {
    const [errors, setErrors] = useState<ValidationResult[]>([]);
    const [touched, setTouched] = useState(false);
    const debouncedValidate = useDebouncedCallback((value: ContentPropertyValueDto) => {
        postApiValidationProperty(value, {
            contentTypeName: "NewsContent",
            propertyName: propertyName
        }).then((errors) => {
            setErrors(errors.data);
        });
    }, 200);

    useUpdateEffect(() => {
        if (!touched)
            return;

        debouncedValidate(valueDto);
    }, [valueDto.value, touched]);


    const baseProps = {
        label: label,
        variant: "filled",
        value: valueDto.value?.toString() ?? "",
        onChange: (e: React.ChangeEvent<HTMLInputElement>) => {
            setTouched(true);
            onChange(e);
        },
        fullWidth: true,
        error: errors.length > 0,
        helperText: errors[0]?.errorMessage ?? null,
        disabled: valueDto.readOnly || disabled
    } as TextFieldProps;

    switch (valueDto.inputType) {
        case InputType.Input:
            return (
                <>
                    <TextField {...baseProps} />
                </>
            ) 
        case InputType.TextArea:
            return <TextField {...baseProps} multiline rows={7} />
        case InputType.DateTimePicker:
            return <TextField {...baseProps} type="datetime-local" />

        default:
            return <>No template defined for content type</>
    }
}
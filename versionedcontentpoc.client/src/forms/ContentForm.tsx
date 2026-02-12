import { forwardRef, useImperativeHandle, useRef, useState } from "react";
import { InputType, Language, postApiValidationProperty, type ContentPropertyValueDto, type ContentReference, type ValidationResult } from "../api/client";
import { Button, Grid, TextField, type TextFieldProps } from "@mui/material";
import useUpdateEffect from "../hooks/useUpdateEffect";
import { useDebouncedCallback } from "../hooks/useDebouncedCallback";
import ContentPicker from "./ContentPicker";

interface ContentFormProps {
    properties: { [key: string]: ContentPropertyValueDto };
    contentTypeName: string;
    language: Language;
    onChange: (key: string, value: unknown | undefined) => void;
    onSubmit: () => Promise<void>;
    submitText: string;
    disabled?: boolean;
}
export default function ContentForm({ properties, contentTypeName, language, onChange, onSubmit, submitText, disabled }: ContentFormProps) {
    const inputRefs = useRef<FormElementTemplateHandles[]>([]);

    const handleSubmit = async () => {
        if (inputRefs.current.length === 0) return;

        const results = await Promise.all(
            inputRefs.current.map((ref) => ref?.validate())
        );

        const allValid = results.every(Boolean);

        if (!allValid) {
            console.log("Form is invalid! Fix errors before submitting.");
            return;
        }

        console.log("Form valid! Proceed with submission.");
        onSubmit();
    };

    return (
        <form onSubmit={(e) => {
            e.preventDefault();
        }}>
            <Grid container spacing={2}>
                {Object.entries(properties).map(([key], index) => (
                    <Grid size={12} key={key}>
                        <FormElementTemplate
                            ref={(el) => { inputRefs.current[index] = el! }}
                            label={key}
                            contentTypeName={contentTypeName}
                            propertyName={key}
                            valueDto={properties[key]}
                            disabled={disabled}
                            language={language}
                            onChange={(value) => onChange(key, value)}
                        />
                    </Grid>
                ))}
                <Button sx={{ ml: 'auto' }} variant="contained" onClick={handleSubmit}>
                    {submitText}
                </Button>
            </Grid>
        </form>
    );
}


export type FormElementTemplateProps = {
    label: string;
    contentTypeName: string;
    propertyName: string;
    valueDto: ContentPropertyValueDto;
    language: Language;
    onChange: (value: unknown | undefined) => void;
    disabled?: boolean;
}
interface FormElementTemplateHandles { validate: () => Promise<boolean>; }

const FormElementTemplate = forwardRef<FormElementTemplateHandles, FormElementTemplateProps>(
    ({ label, propertyName, contentTypeName, valueDto, disabled, language, onChange }, ref) => {
        const [errors, setErrors] = useState<ValidationResult[]>([]);
        const [touched, setTouched] = useState(false);

        const validate = async () => {
            setTouched(true);
            const res = await postApiValidationProperty(valueDto, {
                contentTypeName: contentTypeName,
                propertyName,
            });
            setErrors(res.data);
            return res.data.length === 0;
        }

        const debouncedValidate = useDebouncedCallback(validate, 200);

        useImperativeHandle(ref, () => ({
            validate: validate,
        }));

        useUpdateEffect(() => {
            if (!touched) return;
            debouncedValidate();
        }, [valueDto.value, touched]);

        const handleChange = (value: unknown | undefined) => {
            setTouched(true);
            onChange(value);
        };

        const baseProps = {
            label: label,
            variant: "filled",
            value: valueDto.value?.toString() ?? "",
            onChange: (e) => { handleChange(e.target.value) },
            fullWidth: true,
            error: errors.length > 0,
            helperText: errors[0]?.errorMessage ?? null,
            disabled: valueDto.readOnly || disabled,
            required: valueDto.isRequired
        } as TextFieldProps;

        switch (valueDto.inputType) {
            case InputType.Input:
                return <TextField {...baseProps} />;
            case InputType.TextArea:
                return <TextField {...baseProps} multiline rows={7} />;
            case InputType.DateTimePicker:
                return <TextField {...baseProps} type="datetime-local" />;
            case InputType.ContentPicker:
                return <ContentPicker label={label} onChange={handleChange} language={language} value={valueDto.value as ContentReference} />

            default:
                return <>No template defined for property "{propertyName}"</>;
        }
    }
);
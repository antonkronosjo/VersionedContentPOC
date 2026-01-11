import type { ChangeEventHandler, JSX } from "react";
import type { ContentPropertyValueDto } from "../api/client";
import { Button, Grid, TextField } from "@mui/material";

interface ContentFormProps {
    properties: { [key: string]: ContentPropertyValueDto };
    onChange: (key: string, value: string) => void;
    onSubmit: () => Promise<void>;
    submitText: string;
}
export default function ContentForm({ properties, onChange, onSubmit, submitText }: ContentFormProps) {
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

const resolveTemplate = (label: string, propertyValue: ContentPropertyValueDto, onChange: ChangeEventHandler<HTMLInputElement>): JSX.Element => {
    switch (propertyValue.propertyTypeFullName) {
        case "System.String":
            return <TextField label={label} value={propertyValue.value?.toString() ?? ""} onChange={onChange} fullWidth />
        case "System.DateTime":
            return <TextField placeholder="a" type="datetime-local" label={label} value={propertyValue.value?.toString() ?? ""} onChange={onChange} fullWidth />
        default:
            return <>No template defined for content type</>
    }
}
import { FormControl, InputLabel, Select, MenuItem, type SelectChangeEvent } from "@mui/material"
import { useId } from "react";

interface ContentTypeSelectorProps {
    value: string | null | undefined;
    contentTypes: string[];
    onChange?: (event: SelectChangeEvent) => void;
    includeNull?: boolean;
}
export default function ContentTypeSelector({ value, contentTypes, includeNull, onChange }: ContentTypeSelectorProps)
{
    const labelId = useId();

    return (
        <FormControl variant="filled" fullWidth>
            <InputLabel id={labelId}>Content type</InputLabel>
            <Select
                labelId={labelId}
                label="Content type"
                value={value ?? ""}
                onChange={onChange}
            >
                {includeNull &&
                    <MenuItem value={null}>Show all</MenuItem>
                }
                {contentTypes.map((contentType) => (
                    <MenuItem value={contentType}>{contentType}</MenuItem>
                ))}
            </Select>
        </FormControl>
    );
}
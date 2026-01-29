import { FormControl, InputLabel, Select, MenuItem, type SelectChangeEvent } from "@mui/material"
import { useId } from "react";

interface ContentTypeSelectorProps {
    value: string | null | undefined;
    contentTypes: string[];
    onChange?: (event: SelectChangeEvent) => void;
    displayEmpty?: boolean;
}
export default function ContentTypeSelector({ value, contentTypes, displayEmpty, onChange }: ContentTypeSelectorProps)
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
                displayEmpty={displayEmpty}
            >
                {displayEmpty &&
                    <MenuItem value={undefined}>Show all</MenuItem>
                }
                {contentTypes.map((contentType) => (
                    <MenuItem value={contentType}>{contentType}</MenuItem>
                ))}
            </Select>
        </FormControl>
    );
}
import { FormControl, InputLabel, Select, MenuItem, type SelectChangeEvent } from "@mui/material"
import { Language } from "../api/client";
import { useId } from "react";

interface LanguageSelectorProps {
    value: Language | ""
    displayEmpty?: boolean
    onChange?: (event: SelectChangeEvent) => void;
}
export default function LanguageSelector({ value, displayEmpty, onChange }: LanguageSelectorProps)
{
    const labelId = useId();

    return (
        <FormControl fullWidth variant="filled">
            <InputLabel id={labelId}>Select language</InputLabel>
                <Select
                    labelId={labelId}
                    label="Language"
                    value={value}
                    onChange={onChange}
                    displayEmpty={displayEmpty}
            >

                {displayEmpty &&
                    <MenuItem value={undefined}>Show all</MenuItem>
                }
                {Object.values(Language).map(currLang => (
                    <MenuItem key={currLang} value={currLang}>
                        { currLang }
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
}
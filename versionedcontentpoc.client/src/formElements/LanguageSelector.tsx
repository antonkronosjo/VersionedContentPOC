import { FormControl, InputLabel, Select, MenuItem, type SelectChangeEvent } from "@mui/material"
import { Language } from "../api/client";
import { useId } from "react";

interface LanguageSelectorProps {
    value: Language | ""
    onChange?: (event: SelectChangeEvent) => void;
}
export default function LanguageSelector({ value, onChange }: LanguageSelectorProps)
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
                >
            {
                Object.values(Language).map(currLang => (
                    <MenuItem key={currLang} value={currLang}>
                        { currLang }
                    </MenuItem>
                ))
            }
            </Select>
        </FormControl>
    );
}
import { Button, MenuItem, Menu } from "@mui/material";
import { Language } from "../api/client";
import { useState } from "react";

interface LanguageSelectButtonProps {
    text: string,
    languages: Language[] | null,
    handleSelect: (value: Language) => void
}
export default function LanguageSelectButton({ text, languages, handleSelect }: LanguageSelectButtonProps) {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    if (!languages)
        return (<></>);

    return (
        <>
            <Button
                aria-controls={open ? "simple-menu" : undefined}
                aria-haspopup="true"
                onClick={handleClick}
                variant="text"
            >
                {text}
            </Button>
            <Menu
                id="simple-menu"
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
            >
                {Object.values(Language)
                    .filter(x => !languages?.includes(x) && x !== Language.Invariant)
                    .map(language => (
                        <MenuItem onClick={() => {
                            handleSelect(language);
                            handleClose();
                        }}>
                            {language}
                        </MenuItem>
                    ))}
            </Menu>
        </>
    );
}
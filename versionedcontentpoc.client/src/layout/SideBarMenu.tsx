import { Box, List, ListItemButton, ListItemIcon, ListItemText, Paper } from '@mui/material';
import { Home, Add, LibraryBooks } from '@mui/icons-material';
import { Link as RouterLink, useLocation } from 'react-router-dom';
import { routes } from '../services/routeResolver';
import type { JSX } from 'react';

export default function SidebarMenu() {
    const location = useLocation();
    const menuItems = getMenuItems(location.pathname);


    return (
        <Paper>
            <Box component="nav">
                <List disablePadding>
                    {menuItems.map((menuItem) => (
                        <ListItemButton component={RouterLink} to={menuItem.url} selected={location.pathname === menuItem.url}>
                            <ListItemIcon>
                                {menuItem.icon}
                            </ListItemIcon>
                            <ListItemText primary={menuItem.text} />
                        </ListItemButton>
                    ))}
                </List>
            </Box>
        </Paper>
    );
}

const getMenuItems = (pathName: string):MenuItem[] => {
    if (pathName.startsWith("/cms"))
        return [
            {
                text: "Manage content",
                icon: <LibraryBooks />,
                url: routes.cmshome
            },
            {
                text: "Add content",
                icon: <Add />,
                url: routes.create
            },
        ];

    return [
        {
            text: "Home",
            icon: <Home />,
            url: routes.home
        }
    ];

}

interface MenuItem {
    text: string;
    icon: JSX.Element;
    url: string;
}
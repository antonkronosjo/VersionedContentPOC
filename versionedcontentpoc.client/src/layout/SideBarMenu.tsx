import React from 'react';
import { Box, List, ListItemButton, ListItemIcon, ListItemText, Paper } from '@mui/material';
import { Home, PlusOne, Info } from '@mui/icons-material';
import { Link as RouterLink } from 'react-router-dom';
import { routes } from '../services/routeResolver';

export default function SidebarMenu() {
    return (
        <Paper>
            <Box component="nav">
                <List disablePadding>
                    <ListItemButton component={RouterLink} to={routes.home}>
                        <ListItemIcon>
                            <Home />
                        </ListItemIcon>
                        <ListItemText primary="Home" />
                    </ListItemButton>
                    <ListItemButton component={RouterLink} to={routes.select}>
                        <ListItemIcon>
                            <PlusOne />
                        </ListItemIcon>
                        <ListItemText primary="Create" />
                    </ListItemButton>
                </List>
            </Box>
        </Paper>
    );
}
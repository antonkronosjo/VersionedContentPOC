import { BrowserRouter as Router, Routes, Route, Link as RouterLink } from 'react-router-dom';
import './App.css'
import HomePage from './pages/HomePage';
import CreateContentPage from './pages/CreateContentPage';
import SelectContentPage from './pages/SelectContentPage';
import UpdateContentPage from './pages/UpdateContentPage';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { AppBar, Box, Container, Grid, Toolbar, Typography } from '@mui/material';
import SideBarMenu from './layout/SideBarMenu';
import { routes } from './services/routeResolver';

const appTheme = createTheme({
    palette: {
        mode: 'dark', // This enables dark mode
    },
    typography: {
        // smaller headings but still proportional
        h1: { fontSize: '2rem', fontWeight: 600 },  // ~32px
        h2: { fontSize: '1.75rem', fontWeight: 600 }, // ~28px
        h3: { fontSize: '1.5rem', fontWeight: 600 }, // ~24px
        h4: { fontSize: '1.25rem', fontWeight: 600 }, // ~20px
        h5: { fontSize: '1rem', fontWeight: 600 }, // ~16px
        h6: { fontSize: '0.875rem', fontWeight: 600 }, // ~14px
        body1: { fontSize: '0.9375rem' }, // ~15px body
        body2: { fontSize: '0.875rem' },  // ~14px secondary text
    },
});

function App() {
    return (
        <ThemeProvider theme={appTheme}>
            <CssBaseline />
            <Router>
                <AppBar position="static" >
                    <Toolbar>
                        <Typography variant="h1">VersionedContentPOC</Typography>
                    </Toolbar>
                </AppBar>
                <Container maxWidth={false} sx={{ flex: 1, mb: 1, mt: 1, pl: 1, pr: 1 }} disableGutters>
                    <Grid container spacing={1}>
                        <Grid size={2}>
                            <SideBarMenu />
                        </Grid>
                        <Grid size={10}>
                            <AppRoutes />
                        </Grid>
                    </Grid>
                </Container>
                {/* Footer */}
                <Box component="footer" sx={{ py: 2, textAlign: 'center', bgcolor: 'grey.900' }}>
                    <Typography variant="body2">VersionedContentPOC</Typography>
                </Box>
            </Router>
        </ThemeProvider>
  )
}

export default App

function AppRoutes() {
    return (
        <Routes>
            <Route path={routes.home} element={<HomePage />} />
            <Route path={routes.select} element={<SelectContentPage />} />
            <Route path={routes.create.pattern()} element={<CreateContentPage />} />
            <Route path={routes.update.pattern()} element={<UpdateContentPage />} />
        </Routes>
    );
}
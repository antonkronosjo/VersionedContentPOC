import { Language, useGetApiContentsummaryContentroots, type ContentRootSummary } from "../../api/client"
import { Link as RouterLink, useSearchParams } from "react-router-dom";
import { IconButton, Typography, Grid, Paper, FormControl, InputLabel, Select, MenuItem, FormControlLabel, Checkbox, TableContainer, Table, TableBody, TableCell, TableHead, TableRow, Link } from "@mui/material";
import { Edit } from '@mui/icons-material';
import { routes } from "../../services/routeResolver";
import { format } from "date-fns";


export default function CMSHomePage() {
    const [searchParams] = useSearchParams();
    const contentType = searchParams.get("contentType") as string | undefined;
    const published = searchParams.get("published") === "true";
    const { data, isLoading, error } = useGetApiContentsummaryContentroots({ ContentType: contentType, Published: published });
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (
        <Grid container>
            <Grid size={9}>
                <Paper sx={{ p: 1 }} >
                    <Typography variant="h4" component="h1" gutterBottom>
                        Manage content
                    </Typography>
                    <ContentFilter />
                    {data?.data?.length 
                        ? <ContentRootTable contentRoots={data?.data} />
                        : <i>No content exists...</i>
                    }
                </Paper>
            </Grid>
        </Grid>

    );
}

function ContentFilter() {
    const [searchParams, setSearchParams] = useSearchParams();
    const language = (searchParams.get("language") as Language) ?? Language.SV;
    const published = searchParams.get("published") === "true";

    const setQueryParam = (key: string, value: string | null) => {
        setSearchParams(prev => {
            const params = new URLSearchParams(prev);
            if (value === null) params.delete(key);
            else params.set(key, value);
            return params;
        });
    };

    return (
        <div style={{ display: "flex", gap: 16, marginTop: 16 }}>
            {/* Language Select */}
            <FormControl sx={{ minWidth: 200 }}>
                <InputLabel id="language-select-label">View content on language</InputLabel>
                <Select
                    labelId="language-select-label"
                    value={language}
                    onChange={e => setQueryParam("language", e.target.value)}
                    label="View content on language"
                >
                    {Object.values(Language).map(currLang => (
                        <MenuItem key={currLang} value={currLang}>
                            {currLang}
                        </MenuItem>
                    ))}
                </Select>
            </FormControl>

            {/* Published Checkbox */}
            <FormControlLabel
                control={
                    <Checkbox
                        checked={published}
                        onChange={e => setQueryParam("published", e.target.checked ? "true" : null)}
                    />
                }
                label="Published only"
            />
        </div>
    );
};
interface ContentRootTableProps {
    contentRoots: ContentRootSummary[]
}
function ContentRootTable({ contentRoots }: ContentRootTableProps) {
    return (
        <TableContainer>
            <Table sx={{ minWidth: 650 }} aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell>Content Id</TableCell>
                        <TableCell>Content type</TableCell>
                        <TableCell>Languages</TableCell>
                        <TableCell>Start publish</TableCell>
                        <TableCell>Stop publish</TableCell>
                        <TableCell>Last updated</TableCell>
                        <TableCell></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {contentRoots.map((contentRoot) => (
                        <ContentRootTableRow contentRoot={contentRoot} />
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}
interface ContentRootTableRowProps {
    contentRoot: ContentRootSummary;
}
function ContentRootTableRow({ contentRoot }: ContentRootTableRowProps) {
    return (
        <TableRow sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
            <TableCell component="th" scope="row">
                {contentRoot.contentId}
            </TableCell>
            <TableCell align="left">{contentRoot.contentTypeName}</TableCell>
            <TableCell align="left">
                {contentRoot.languageVersions?.length &&
                    contentRoot.languageVersions.map((lang, index) => (
                        <span key={lang}>
                            {index > 0 && ", "}
                            <Link component={RouterLink} to={routes.edit.build({ contentId: contentRoot.contentId, language: lang })}>
                                {lang}
                            </Link>
                        </span>
                    ))
                }
            </TableCell>
            <TableCell align="left">{formatDateString(contentRoot.startPublish, "yyyy-MM-dd HH:mm")}</TableCell>
            <TableCell align="left">{formatDateString(contentRoot.stopPublish, "yyyy-MM-dd HH:mm")}</TableCell>
            <TableCell align="left">{formatDateString(contentRoot.stopPublish, "yyyy-MM-dd HH:mm")}</TableCell>
            <TableCell align="left">
                <IconButton
                    aria-label="edit"
                    component={RouterLink}
                    to={routes.edit.build({
                        contentId: contentRoot.contentId,
                        language: Language.SV //Todo: This should not be a mandatory param
                    })}
                >
                    {<Edit fontSize="small" />}
                </IconButton>
            </TableCell>
        </TableRow>
    );
}

const formatDateString = (dateString: string | null | undefined, dateFormat: string):string => {
    if (!dateString)
        return "-";

    return format(new Date(dateString), dateFormat);
}
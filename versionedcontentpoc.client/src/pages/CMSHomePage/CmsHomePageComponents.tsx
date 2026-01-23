import { Language, useGetApiContentTypes, type ContentRootSummary } from "../../api/client"
import { Link as RouterLink, useSearchParams } from "react-router-dom";
import { Checkbox, FormControlLabel, Grid, Link, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import LanguageSelector from "../../formElements/LanguageSelector";
import ContentTypeSelector from "../../formElements/ContentTypeSelector";
import { relativeDateTime } from "../../utils/dateUtils";
import { routes } from "../../utils/routeResolver";

export function ContentFilter() {
    const { data: response, isLoading, error } = useGetApiContentTypes();
    const [searchParams, setSearchParams] = useSearchParams();
    const language = (searchParams.get("language") as Language) ?? null;
    const contentType = searchParams.get("contentType");
    const published = searchParams.get("published") === "true";

    if (error)
        return (<p>Error</p>);

    if (!response || isLoading)
        return (<p>Loading...</p>);

    const setQueryParam = (key: string, value: string | null) => {
        setSearchParams(prev => {
            const params = new URLSearchParams(prev);
            if (value === null) params.delete(key);
            else params.set(key, value);
            return params;
        });
    };

    return (
        <Grid container spacing={2}>
            <Grid size={4}>
                <ContentTypeSelector includeNull value={contentType} contentTypes={response.data} onChange={(e) => { setQueryParam("contentType", e.target.value) }} />
            </Grid>
            <Grid size={4}>
                <LanguageSelector value={language} onChange={(e) => { setQueryParam("language", e.target.value) }} />
            </Grid>
            <Grid size={4}>
                <FormControlLabel
                    control={
                        <Checkbox
                            checked={published}
                            onChange={e => setQueryParam("published", e.target.checked ? "true" : null)}
                        />
                    }
                    label="Published"
                />
            </Grid>
        </Grid>
    );
};

interface ContentRootTableProps {
    contentRoots: ContentRootSummary[]
}
export function ContentRootTable({ contentRoots }: ContentRootTableProps) {
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
            <TableCell align="left">{relativeDateTime(contentRoot.startPublish)}</TableCell>
            <TableCell align="left">{relativeDateTime(contentRoot.stopPublish)}</TableCell>
            <TableCell align="left">{relativeDateTime(contentRoot.stopPublish)}</TableCell>
        </TableRow>
    );
}
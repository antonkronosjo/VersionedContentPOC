import { Language, useGetApiContentUpdateschema, getGetApiContentUpdateschemaQueryKey } from "../../api/client";
import { Box, Grid, Paper, Typography } from "@mui/material";
import ContentVersionsList from "../../compontents/ContentVersionsList";
import { useQueryClient } from '@tanstack/react-query';
import { EditContentPageHeader, EditContentForm, LanguageBranchTabs } from "./EditContentPageComponents";
import { useTypedParams } from "../../hooks/useTypedParams";
import { routes } from "../../utils/routeResolver";
import { useNavigate } from "react-router-dom";

export default function EditContentPage() {
    const { contentId, language, versionId } = useTypedParams({
        contentId: Number,
        language: (v) => v as Language,
        versionId: (v) => v ? Number(v) : undefined,
    });
    const navigate = useNavigate();

    const { data: response, isLoading, error } = useGetApiContentUpdateschema({ contentId: contentId, language: language, versionId: versionId });
    const queryClient = useQueryClient();

    if (isLoading || !response)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);
    
    const refetch = () => {
        queryClient.invalidateQueries({
            queryKey: getGetApiContentUpdateschemaQueryKey({ contentId, language: language })
        });
    };
    
    return (    
        <Grid container spacing={1} alignItems="flex-start">
            <Grid size={12}>
                <Paper sx={{ p: 1 }}>
                    <EditContentPageHeader
                        metadata={response.data.metadata}
                        refetch={refetch}
                    />
                </Paper>
            </Grid>
            <Grid size={8}>
                <Paper>
                    <LanguageBranchTabs metadata={response.data.metadata} />
                    <Box sx={{ p: 1 }}>
                        <EditContentForm
                            key={response.data.metadata.versionId + response.data.metadata.language}
                            schema={response.data}
                            onSubmit={(content) => {
                                navigate(routes.edit.build({
                                    contentId: contentId.toString(),
                                    language: language,
                                    versionId: content.versionId.toString()
                                }))
                            }}
                        />
                    </Box>
                </Paper>
            </Grid>
            <Grid size={4}>
                <Paper sx={{ p: 1 }}>
                    <Typography variant="h5" component="h2">
                        Version history
                    </Typography>
                    <ContentVersionsList
                        key={response.data.metadata.versionId + response.data.metadata.status}
                        contentId={contentId}
                        versionId={response.data.metadata.versionId}
                        language={language!}
                        onUpdate={refetch}
                    />
                </Paper>
            </Grid>
            
        </Grid>
        
    );
}
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
                            schema={response.data}
                            onSubmit={() => {
                                refetch();
                                navigate(routes.edit.build({
                                    contentId: contentId.toString(),
                                    language: language
                                }))
                            }}
                            versionId={versionId}
                            activeVersionId={response.data.metadata.activeVersionId}
                            key={response.data.metadata.activeVersionId}
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
                        contentId={contentId}
                        versionId={response.data.metadata.versionId}
                        language={language!}                       
                        onUpdate={refetch}
                        key={response.data.metadata.activeVersionId}
                    />
                </Paper>
            </Grid>
            
        </Grid>
        
    );
}
import { useGetApiContentAll } from ".././api/client"

function HomePage() {
    const { data, isLoading, error } = useGetApiContentAll();
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (

        <>
            <h1>HOME</h1>
            {data?.data.map((content) => (
                <p>{content.contentId}</p>
            ))}
        </>
        
    );
}

export default HomePage;
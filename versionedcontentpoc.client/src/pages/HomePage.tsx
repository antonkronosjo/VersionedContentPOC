import type { JSX } from "react";
import { useGetApiContentAll, type EventContent, type GetApiContentAll200Item, type NewsContent } from ".././api/client"


function HomePage() {
    const { data, isLoading, error } = useGetApiContentAll();
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    return (

        <>
            <h1>HOME - VersionedContentPOC</h1>
            {data?.data.map((content) => (
                <div className={"contentTemplate " + content.contentType}>
                    <p style={{ fontSize: ".8em" }}>ContentType: {content.contentType}</p>
                    <p style={{ fontSize: ".8em" }}>Created: {content.contentRoot?.created}</p>
                    {resolveTemplate(content)}
                </div>
            ))}
        </>
        
    );
}

export default HomePage;

const NewsTemplate = ({ content }: { content: NewsContent }) => (
    <>
        <h2>{content.heading}</h2>
        <p>{content.lead}</p>
        <p>{content.text}</p>
    </>
);

const EventTemplate = ({ content }: { content: EventContent }) => (
    <>
        <h2>{content.heading}</h2>
        <p><strong>Start:</strong> {content.startDate}</p>
        <p><strong>End:</strong> {content.endDate}</p>
    </>
);

const resolveTemplate = (content: GetApiContentAll200Item): JSX.Element =>  {
    switch (content.contentType) {
        case "EventContent":
            return <EventTemplate content={content} />;
        case "NewsContent":
            return <NewsTemplate content={content} />;
    }
}
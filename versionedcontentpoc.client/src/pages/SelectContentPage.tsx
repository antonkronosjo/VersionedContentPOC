import { useGetApiContentTypes } from "../api/client"
import { useNavigate } from "react-router-dom";

function SelectContentPage() {
    const { data, isLoading, error } = useGetApiContentTypes();
    const navigate = useNavigate();
    
    if (isLoading)
        return (<p>Is loading</p>);

    if (error)
        return (<p>Error</p>);

    console.warn("SELECT CONTENT", data.data);

    return (
        <>
            <h1>Select content type</h1>
            <select onChange={(e) => {
                if (e.target.value) {
                    navigate("/create/" + e.target.value);
                }
            }}>
                <option value="">Select content type...</option>
                {data?.data.map((contentType) => (
                    <option value={contentType}>{contentType}</option>
                ))}
            </select>
        </>
        
    );
}

export default SelectContentPage;
import type { ChangeEventHandler, JSX } from "react";
import type { ContentPropertyValueDto } from "../api/client";

interface ContentFormProps {
    properties: { [key: string]: ContentPropertyValueDto };
    onChange: (key: string, value: string) => void;
    onSubmit: () => Promise<void>;
    submitText: string;
}
export default function ContentForm({ properties, onChange, onSubmit, submitText }: ContentFormProps) {
    return (
        <>
            {Object.entries(properties).map(([key]) => (
                <div key={key}>
                    <label>{key}</label><br />
                    {resolveTemplate(properties[key], (e) => { onChange(key, e.target.value) })}
                </div>
            ))}
            <button onClick={onSubmit}>{submitText}</button>
        </>
    );
}

const resolveTemplate = (propertyValue: ContentPropertyValueDto, onChange: ChangeEventHandler<HTMLInputElement>): JSX.Element => {
    switch (propertyValue.propertyTypeFullName) {
        case "System.String":
            return <input value={propertyValue.value?.toString() ?? ""} onChange={onChange} />
        case "System.DateTime":
            return <input type="datetime-local" value={propertyValue.value?.toString() ?? ""} onChange={onChange} />
        default:
            return <>No template defined for content type</>
    }
}
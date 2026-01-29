import { useParams } from "react-router-dom";

type ParamParsers<T> = {
    [K in keyof T]: (value: string | undefined) => T[K];
};

export function useTypedParams<T>(
    parsers: ParamParsers<T>
): T {
    const params = useParams();
    const result = {} as T;

    for (const key in parsers) {
        result[key] = parsers[key](params[key]);
    }

    return result;
}
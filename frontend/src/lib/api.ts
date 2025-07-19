export type ApiMethod = 'GET' | 'POST' | 'PUT' | 'DELETE';

interface ApiRequestOptions {
  method?: ApiMethod;
  body?: any;
  params?: Record<string, string>;
  headers?: HeadersInit;
  withCredentials?: boolean;
}

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

if (!BASE_URL) {
  throw new Error('❌ BASE_URL is not defined in .env.local');
}

export async function apiRequest<TResponse = any>(
  endpoint: string,
  options: ApiRequestOptions = {}
): Promise<TResponse> {
  const {
    method = 'GET',
    body,
    params,
    headers,
    withCredentials = true,
  } = options;

  // ✅ Construct full URL
  const url = new URL(endpoint, BASE_URL);

  // ✅ Add query parameters if any
  if (params) {
    Object.entries(params).forEach(([key, value]) =>
      url.searchParams.append(key, value)
    );
  }

  const fetchOptions: RequestInit = {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...headers,
    },
    credentials: withCredentials ? 'include' : 'same-origin',
  };

  if (body && method !== 'GET') {
    fetchOptions.body = JSON.stringify(body);
  }

  const res = await fetch(url.toString(), fetchOptions);

  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(errorText || '❌ API request failed');
  }

  // ✅ Handle 204 No Content
  if (res.status === 204) return {} as TResponse;

  return res.json();
}

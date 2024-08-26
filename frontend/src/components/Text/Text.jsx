import styled from 'styled-components';

const TextWrapper = styled.div`
    font-size: 14px;
    color: rgba(255, 255, 255, 0.5);
`;

export default function Text({ children }) {
    return <TextWrapper>{children} </TextWrapper>;
}

import React, { useEffect, useState } from 'react';
import FastImage from 'react-native-fast-image';
import { Assets } from '@app/assets';

const Image = ({ source, headers = {}, style = {}, resizeMode = FastImage.resizeMode.cover, tintColor = '' }: ImageProps) => {

    const [isLoading, setLoading] = useState<boolean>(true)
    const [errorMessage, setErrorMessage] = useState<boolean>(false)
    const [localSource, setLocalSource] = useState<ImageSource | number>(source)

    useEffect(() => {
        setLocalSource(source)
    }, [source])

    const onLoadStart = () => {
        setLoading(true)
        setErrorMessage(false)
    }

    const onLoadEnd = () => {
        setLoading(false)
    }

    const onError = () => {
        setLoading(false)
        setErrorMessage(true)
        setLocalSource(Assets.noProfile)
    }

    return (
        <>
            <FastImage
                style={style}
                tintColor={tintColor}
                onError={onError}
                onLoadEnd={onLoadEnd}
                onLoadStart={onLoadStart}
                source={
                    typeof localSource === 'object' && 'uri' in localSource
                        ? {
                            uri: localSource.uri,
                            headers,
                            priority: FastImage.priority.high,
                        } : localSource}
                resizeMode={resizeMode}
            />
        </>
    );
}

export default Image;

interface ImageSource {
    uri?: string;
}

interface ImageProps {
    source: ImageSource | number,
    headers?: { [key: string]: string };
    style: object,
    resizeMode?: keyof typeof FastImage.resizeMode;
    tintColor?: string
}
